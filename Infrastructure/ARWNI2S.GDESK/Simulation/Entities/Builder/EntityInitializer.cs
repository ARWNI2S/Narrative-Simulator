namespace ARWNI2S.Engine.Simulation.Entities.Builder
{
    /** FObjectInitializer options */
    public enum EntityInitializerOptions
    {
        None = 0,
        CopyClass = 1, // copy transient from the class defaults instead of the pass in archetype ptr
        InitializeProperties = 2, // initialize property values with the archetype values
    };


    public class EntityInitializer
    {
        //private EntityHierarchyBuilder _hierachyBuilder;

        public EntityInitializer()
        {
            //_hierachyBuilder = new EntityHierarchyBuilder();
        }

        public EntityInitializer(ISimulableEntity entity, ISimulableEntity? archetype, EntityInitializerOptions options, EntityInstancingGraph? instanceGraph = null)
        {

        }

        //CreateDefaultSubobject



        //internal IActorComponent GetRootObject()
        //{
        //    return RootNode.Value;
        //}

        //internal bool SetRootObject(IActorComponent rootComponent)
        //{
        //    try
        //    {
        //        RootNode.Value = rootComponent;
        //        return true;
        //    }
        //    catch
        //    {
        //        //TODO: HANDLE ERROR
        //    }
        //    return false;
        //}
    }
}


//class FObjectInitializer
//{
//    /**
//	 * Constructor
//	 * @param	InObj object to initialize, from static allocate object, after construction
//	 * @param	InObjectArchetype object to initialize properties from
//	 * @param	InOptions initialization options, see EObjectInitializerOptions
//	 * @param	InInstanceGraph passed instance graph
//	 */
//    COREUOBJECT_API FObjectInitializer(UObject* InObj, UObject* InObjectArchetype, EObjectInitializerOptions InOptions, struct FObjectInstancingGraph* InInstanceGraph = nullptr);

//	UE_DEPRECATED(5.0, "Use version that takes EObjectInitializerOptions")

//    FObjectInitializer(UObject* InObj, UObject* InObjectArchetype, bool bInCopyTransientsFromClassDefaults, bool bInShouldInitializeProps, struct FObjectInstancingGraph* InInstanceGraph = nullptr)
//		: FObjectInitializer(InObj, InObjectArchetype,
//            (bInCopyTransientsFromClassDefaults? EObjectInitializerOptions::CopyTransientsFromClassDefaults : EObjectInitializerOptions::None) |
//			(bInShouldInitializeProps? EObjectInitializerOptions::InitializeProperties : EObjectInitializerOptions::None),
//			InInstanceGraph)
//	{
//	}

//	/** Special constructor for static construct object internal that passes along the params block directly */
//	COREUOBJECT_API FObjectInitializer(UObject* InObj, const FStaticConstructObjectParameters& StaticConstructParams);

//private:
//	/** Helper for the common behaviors in the constructors */
//	COREUOBJECT_API void Construct_Internal();

//    public:
//	COREUOBJECT_API ~FObjectInitializer();

//    /** 
//	 * Return the archetype that this object will copy properties from later
//	**/
//    FORCEINLINE UObject* GetArchetype() const
//	{
//		return ObjectArchetype;
//	}

// /**
//* Return the object that is being constructed
//**/
//FORCEINLINE UObject* GetObj() const
//	{
//		return Obj;
//	}

//	FORCEINLINE struct FObjectInstancingGraph*GetInstancingGraph()

//    {
//    return InstanceGraph;
//}

// /**
//* Return the class of the object that is being constructed
//**/
//COREUOBJECT_API UClass* GetClass() const;

// /**
// * Create a component or subobject that will be instanced inside all instances of this class.
// * @param	TReturnType					class of return type, all overrides must be of this type
// * @param	Outer						outer to construct the subobject in
// * @param	SubobjectName				name of the new component, this will be the same for all instances of this class
// * @param	bTransient					true if the component is being assigned to a transient property
// */
//template <class TReturnType>
//TReturnType * CreateDefaultSubobject(UObject * Outer, FName SubobjectName, bool bTransient = false) const
//	{
//		UClass* ReturnType = TReturnType::StaticClass();
//return static_cast<TReturnType*>(CreateDefaultSubobject(Outer, SubobjectName, ReturnType, ReturnType, /*bIsRequired =*/ true, bTransient));
//	}

//	/**
//	 * Create optional component or subobject. Optional subobjects will not get created.
//	 * if a derived class specifies DoNotCreateDefaultSubobject with the subobject name.
//	 * @param	TReturnType					class of return type, all overrides must be of this type
//	 * @param	Outer						outer to construct the subobject in
//	 * @param	SubobjectName				name of the new component, this will be the same for all instances of this class
//	 * @param	bTransient					true if the component is being assigned to a transient property
//	 */
//	template <class TReturnType>
//    TReturnType * CreateOptionalDefaultSubobject(UObject * Outer, FName SubobjectName, bool bTransient = false) const
//	{
//		UClass* ReturnType = TReturnType::StaticClass();
//return static_cast<TReturnType*>(CreateDefaultSubobject(Outer, SubobjectName, ReturnType, ReturnType, /*bIsRequired =*/ false, bTransient));
//	}

//	/** 
//	 * Create a component or subobject, allows creating a child class and returning the parent class.
//	 * @param	TReturnType					class of return type, all overrides must be of this type 
//	 * @param	TClassToConstructByDefault	class to construct by default
//	 * @param	Outer						outer to construct the subobject in
//	 * @param	SubobjectName				name of the new component, this will be the same for all instances of this class
//	 * @param	bTransient					true if the component is being assigned to a transient property
//	 */ 
//	template <class TReturnType, class TClassToConstructByDefault>
//    TReturnType * CreateDefaultSubobject(UObject * Outer, FName SubobjectName, bool bTransient = false) const
//	{ 
//		return static_cast<TReturnType*>(CreateDefaultSubobject(Outer, SubobjectName, TReturnType::StaticClass(), TClassToConstructByDefault::StaticClass(), /*bIsRequired =*/ true, bTransient));
//	}

//	/**
//	 * Create a component or subobject only to be used with the editor.
//	 * @param	TReturnType					class of return type, all overrides must be of this type
//	 * @param	Outer						outer to construct the subobject in
//	 * @param	SubobjectName				name of the new component, this will be the same for all instances of this class
//	 * @param	bTransient					true if the component is being assigned to a transient property
//	 */
//	template <class TReturnType>
//    TReturnType * CreateEditorOnlyDefaultSubobject(UObject * Outer, FName SubobjectName, bool bTransient = false) const
//	{
//		const UClass* ReturnType = TReturnType::StaticClass();
//return static_cast<TReturnType*>(CreateEditorOnlyDefaultSubobject(Outer, SubobjectName, ReturnType, bTransient));
//	}

//	/**
//	* Create a component or subobject only to be used with the editor.
//	* @param	Outer						outer to construct the subobject in
//	* @param	SubobjectName				name of the new component, this will be the same for all instances of this class
//	* @param	ReturnType					type of the new component
//	* @param	bTransient					true if the component is being assigned to a transient property
//	*/
//	COREUOBJECT_API UObject* CreateEditorOnlyDefaultSubobject(UObject* Outer, FName SubobjectName, const UClass* ReturnType, bool bTransient = false) const;

// /**
// * Create a component or subobject that will be instanced inside all instances of this class.
// * @param	Outer                       outer to construct the subobject in
// * @param	SubobjectName               name of the new component
// * @param	ReturnType                  class of return type, all overrides must be of this type
// * @param	ClassToConstructByDefault   if the derived class has not overridden, create a component of this type
// * @param	bIsRequired                 true if the component is required and will always be created even if DoNotCreateDefaultSubobject was specified.
// * @param	bIsTransient                true if the component is being assigned to a transient property
// */
//COREUOBJECT_API UObject* CreateDefaultSubobject(UObject* Outer, FName SubobjectFName, const UClass* ReturnType, const UClass* ClassToCreateByDefault, bool bIsRequired = true, bool bIsTransient = false) const;

// /**
// * Sets the class to use for a subobject defined in a base class, the class must be a subclass of the class used by the base class.
// * @param	SubobjectName	name of the new component or subobject
// * @param	Class			The class to use for the specified subobject or component.
// */
//const FObjectInitializer& SetDefaultSubobjectClass(FName SubobjectName, const UClass* Class) const
//	{
//		AssertIfSubobjectSetupIsNotAllowed(SubobjectName);
//SubobjectOverrides.Add(SubobjectName, Class);
//return *this;
//	}

//	/**
//	 * Sets the class to use for a subobject defined in a base class, the class must be a subclass of the class used by the base class.
//	 * @param	SubobjectName	name of the new component or subobject
//	 */
//	template <class T>

//    const FObjectInitializer& SetDefaultSubobjectClass(FName SubobjectName) const
//	{
//		return SetDefaultSubobjectClass(SubobjectName, T::StaticClass());
//	}

//	/**
//	 * Indicates that a base class should not create a component
//	 * @param	SubobjectName	name of the new component or subobject to not create
//	 */
//	const FObjectInitializer& DoNotCreateDefaultSubobject(FName SubobjectName) const
//	{
//		AssertIfSubobjectSetupIsNotAllowed(SubobjectName);
//SubobjectOverrides.Add(SubobjectName, nullptr);
//return *this;
//	}

//	/**
//	 * Sets the class to use for a subobject defined in a nested subobject, the class must be a subclass of the class used when calling CreateDefaultSubobject.
//	 * @param	SubobjectName	path to the new component or subobject
//	 * @param	Class			The class to use for the specified subobject or component.
//	 */
//	const FObjectInitializer& SetNestedDefaultSubobjectClass(FStringView SubobjectName, const UClass* Class) const
//	{
//		AssertIfSubobjectSetupIsNotAllowed(SubobjectName);
//SubobjectOverrides.Add(SubobjectName, Class);
//return *this;
//	}

//	/**
//	 * Sets the class to use for a subobject defined in a nested subobject, the class must be a subclass of the class used when calling CreateDefaultSubobject.
//	 * @param	SubobjectName	path to the new component or subobject
//	 * @param	Class			The class to use for the specified subobject or component.
//	 */
//	const FObjectInitializer& SetNestedDefaultSubobjectClass(TArrayView<const FName> SubobjectNames, const UClass* Class) const
//	{
//		AssertIfSubobjectSetupIsNotAllowed(SubobjectNames);
//SubobjectOverrides.Add(SubobjectNames, Class);
//return *this;
//	}

//	/**
//	 * Sets the class to use for a subobject defined in a nested subobject, the class must be a subclass of the class used when calling CreateDefaultSubobject.
//	 * @param	SubobjectName	path to the new component or subobject
//	 */
//	template <class T>

//    const FObjectInitializer& SetNestedDefaultSubobjectClass(FStringView SubobjectName) const
//	{
//		return SetNestedDefaultSubobjectClass(SubobjectName, T::StaticClass());
//	}

//	/**
//	 * Sets the class to use for a subobject defined in a nested subobject, the class must be a subclass of the class used when calling CreateDefaultSubobject.
//	 * @param	SubobjectName	path to the new component or subobject
//	 */
//	template <class T>

//    const FObjectInitializer& SetNestedDefaultSubobjectClass(TArrayView<const FName> SubobjectNames) const
//	{
//		return SetNestedDefaultSubobjectClass(SubobjectNames, T::StaticClass());
//	}

//	/**
//	 * Indicates that a subobject should not create a component if created using CreateOptionalDefaultSubobject
//	 * @param	SubobjectName	name of the new component or subobject to not create
//	 */
//	const FObjectInitializer& DoNotCreateNestedDefaultSubobject(FStringView SubobjectName) const
//	{
//		AssertIfSubobjectSetupIsNotAllowed(SubobjectName);
//SubobjectOverrides.Add(SubobjectName, nullptr);
//return *this;
//	}

//	/**
//	 * Indicates that a subobject should not create a component if created using CreateOptionalDefaultSubobject
//	 * @param	SubobjectName	name of the new component or subobject to not create
//	 */
//	const FObjectInitializer& DoNotCreateNestedDefaultSubobject(TArrayView<const FName> SubobjectNames) const
//	{
//		AssertIfSubobjectSetupIsNotAllowed(SubobjectNames);
//SubobjectOverrides.Add(SubobjectNames, nullptr);
//return *this;
//	}

//	/**
//	 * Asserts with the specified message if code is executed inside UObject constructor
//	 **/
//	static COREUOBJECT_API void AssertIfInConstructor(UObject* Outer, const TCHAR* ErrorMessage);

//FORCEINLINE void FinalizeSubobjectClassInitialization()
//	{
//		bSubobjectClassInitializationAllowed = false;
//	}

//	/** Gets ObjectInitializer for the currently constructed object. Can only be used inside of a constructor of UObject-derived class. */
//	static COREUOBJECT_API FObjectInitializer& Get();

//private:

//	friend class UObject;
//friend class FScriptIntegrationObjectHelper;

//template <class T>
//friend void InternalConstructor(const class FObjectInitializer&X);

// /**
// * Binary initialize object properties to zero or defaults.
// *
// * @param	Obj					object to initialize data for
// * @param	DefaultsClass		the class to use for initializing the data
// * @param	DefaultData			the buffer containing the source data for the initialization
// * @param	bCopyTransientsFromClassDefaults if true, copy the transients from the DefaultsClass defaults, otherwise copy the transients from DefaultData
// */
//static COREUOBJECT_API void InitProperties(UObject* Obj, UClass* DefaultsClass, UObject* DefaultData, bool bCopyTransientsFromClassDefaults);

//COREUOBJECT_API bool IsInstancingAllowed() const;

// /**
// * Calls InitProperties for any default subobjects created through this ObjectInitializer.
// * @param bAllowInstancing	Indicates whether the object's components may be copied from their templates.
// * @return true if there are any subobjects which require instancing.
//*/
//COREUOBJECT_API bool InitSubobjectProperties(bool bAllowInstancing) const;

// /**
// * Create copies of the object's components from their templates.
// * @param Class						Class of the object we are initializing
// * @param bNeedInstancing			Indicates whether the object's components need to be instanced
// * @param bNeedSubobjectInstancing	Indicates whether subobjects of the object's components need to be instanced
// */
//COREUOBJECT_API void InstanceSubobjects(UClass* Class, bool bNeedInstancing, bool bNeedSubobjectInstancing) const;

// /** 
// * Initializes a non-native property, according to the initialization rules. If the property is non-native
// * and does not have a zero contructor, it is inialized with the default value.
// * @param	Property			Property to be initialized
// * @param	Data				Default data
// * @return	Returns true if that property was a non-native one, otherwise false
// */
//static COREUOBJECT_API bool InitNonNativeProperty(FProperty* Property, UObject* Data);

// /**
// * Finalizes a constructed UObject by initializing properties, 
// * instancing/initializing sub-objects, etc.
// */
//COREUOBJECT_API void PostConstructInit();

//private:

//	/**  Little helper struct to manage overrides from derived classes **/
//	struct FOverrides
//{
//    /**  Add an override, make sure it is legal **/
//    COREUOBJECT_API void Add(FName InComponentName, const UClass* InComponentClass, const TArrayView<const FName>* FullPath = nullptr);

//		/**  Add a potentially nested override, make sure it is legal **/
//		COREUOBJECT_API void Add(FStringView InComponentPath, const UClass* InComponentClass);

//    /**  Add a potentially nested override, make sure it is legal **/
//    COREUOBJECT_API void Add(TArrayView<const FName> InComponentPath, const UClass* InComponentClass, const TArrayView<const FName>* FullPath = nullptr);

//		struct FOverrideDetails
//    {
//        const UClass* Class = nullptr;
//        FOverrides* SubOverrides = nullptr;
//    };

//    /**  Retrieve an override, or TClassToConstructByDefault::StaticClass or nullptr if this was removed by a derived class **/
//    FOverrideDetails Get(FName InComponentName, const UClass* ReturnType, const UClass* ClassToConstructByDefault, bool bOptional) const;

//    private:
//		static bool IsLegalOverride(const UClass* DerivedComponentClass, const UClass* BaseComponentClass);

//    /**  Search for an override **/
//    int32 Find(FName InComponentName) const
//		{
//			for (int32 Index = 0 ; Index<Overrides.Num(); Index++)
//			{
//				if (Overrides[Index].ComponentName == InComponentName)
//				{
//					return Index;
//				}
//			}
//			return INDEX_NONE;
//		}
//		/**  Element of the override array **/
//		struct FOverride
//{
//    FName ComponentName;
//    const UClass* ComponentClass = nullptr;
//    TUniquePtr<FOverrides> SubOverrides;
//    bool bDoNotCreate = false;
//    FOverride(FName InComponentName)
//        : ComponentName(InComponentName)
//    {
//    }

//    FOverride& operator=(const FOverride& Other)
//			{
//				ComponentName = Other.ComponentName;
//				ComponentClass = Other.ComponentClass;
//				SubOverrides = (Other.SubOverrides? MakeUnique<FOverrides>(* Other.SubOverrides) : nullptr);
//				bDoNotCreate = Other.bDoNotCreate;
//				return *this;
//			}

//FOverride(const FOverride& Other)
//			{
//				*this = Other;
//			}

//			FOverride(FOverride &&) = default;
//FOverride & operator=(FOverride&&) = default;
//		};
// /**  The override array **/
//TArray < FOverride, TInlineAllocator < 8 > > Overrides;
//	};
// /**  Little helper struct to manage overrides from derived classes **/
//struct FSubobjectsToInit
//{
//    /**  Add a subobject **/
//    void Add(UObject* Subobject, UObject* Template)
//    {
//        for (int32 Index = 0; Index < SubobjectInits.Num(); Index++)
//        {
//            check(SubobjectInits[Index].Subobject != Subobject);
//        }
//        SubobjectInits.Emplace(Subobject, Template);
//    }
//    /**  Element of the SubobjectInits array **/
//    struct FSubobjectInit
//    {
//        UObject* Subobject;
//        UObject* Template;
//        FSubobjectInit(UObject* InSubobject, UObject* InTemplate)
//            : Subobject(InSubobject)
//				, Template(InTemplate)
//        {
//        }
//    };
//    /**  The SubobjectInits array **/
//    TArray<FSubobjectInit, TInlineAllocator<8>> SubobjectInits;
//};

// /** Asserts if SetDefaultSubobjectClass or DoNotCreateOptionalDefaultSuobject are called inside of the constructor body */
//COREUOBJECT_API void AssertIfSubobjectSetupIsNotAllowed(const FName SubobjectName) const;

// /** Asserts if SetDefaultSubobjectClass or DoNotCreateOptionalDefaultSuobject are called inside of the constructor body */
//COREUOBJECT_API void AssertIfSubobjectSetupIsNotAllowed(FStringView SubobjectName) const;

// /** Asserts if SetDefaultSubobjectClass or DoNotCreateOptionalDefaultSuobject are called inside of the constructor body */
//COREUOBJECT_API void AssertIfSubobjectSetupIsNotAllowed(TArrayView<const FName> SubobjectNames) const;

// /**  object to initialize, from static allocate object, after construction **/
//UObject* Obj;
// /**  object to copy properties from **/
//UObject* ObjectArchetype;
// /**  if true, copy the transients from the DefaultsClass defaults, otherwise copy the transients from DefaultData **/
//bool bCopyTransientsFromClassDefaults;
// /**  If true, initialize the properties **/
//bool bShouldInitializePropsFromArchetype;
// /**  Only true until ObjectInitializer has not reached the base UObject class */
//bool bSubobjectClassInitializationAllowed = true;
//#if USE_CIRCULAR_DEPENDENCY_LOAD_DEFERRING
//	/**  */
//	bool bIsDeferredInitializer = false;
//#endif // USE_CIRCULAR_DEPENDENCY_LOAD_DEFERRING
// /**  Instance graph **/
//struct FObjectInstancingGraph*InstanceGraph;
// /**  List of component classes to override from derived classes **/
//mutable FOverrides SubobjectOverrides;
// /**  List of component classes to initialize after the C++ constructors **/
//mutable FSubobjectsToInit ComponentInits;
//#if !UE_BUILD_SHIPPING
// /** List of all subobject names constructed for this object */
//mutable TArray<FName, TInlineAllocator<8>> ConstructedSubobjects;
//#endif
// /**  Previously constructed object in the callstack */
//UObject* LastConstructedObject = nullptr;

// /** Callback for custom property initialization before PostInitProperties gets called */
//TFunction < void() > PropertyInitCallback;

//friend struct FStaticConstructObjectParameters;

//#if WITH_EDITORONLY_DATA
//	/** Detects when a new GC object was created */
//	COREUOBJECT_API void OnGCObjectCreated(FGCObject* InObject);

//	/** Delegate handle for OnGCObjectCreated callback */
//	FDelegateHandle OnGCObjectCreatedHandle;
//	/** List of FGCObjects created during UObject construction */
//	TArray<FGCObject*> CreatedGCObjects;
//#endif // WITH_EDITORONLY_DATA
//};
